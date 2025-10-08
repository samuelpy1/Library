#!/bin/bash

set -e
    
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' 

echo -e "${YELLOW}Etapa 1: Configurando variáveis...${NC}"

RESOURCE_GROUP="rg-challenge-fiap-557884"
LOCATION="brazilsouth"
DB_SERVER_NAME="sql-motoconnect-557884"
DB_NAME="motoconnectdb"
APP_SERVICE_PLAN="asp-motoconnect-fiap-557884"
WEB_APP_NAME="webapp-motoconnect-557884"
DB_ADMIN_USER=""
DB_ADMIN_PASSWORD=""

echo -e "${GREEN}✓ Variáveis configuradas${NC}\n"

# --- Registrar Resource Providers ---

echo -e "${YELLOW}Etapa 2: Verificando Resource Providers...${NC}"

if ! az provider show --namespace Microsoft.Sql --query "registrationState" -o tsv 2>/dev/null | grep -q "Registered"; then
    echo "Registrando Microsoft.Sql (isso pode demorar alguns minutos)..."
    az provider register --namespace Microsoft.Sql
fi

if ! az provider show --namespace Microsoft.Web --query "registrationState" -o tsv 2>/dev/null | grep -q "Registered"; then
    echo "Registrando Microsoft.Web..."
    az provider register --namespace Microsoft.Web
fi

if ! az provider show --namespace Microsoft.Storage --query "registrationState" -o tsv 2>/dev/null | grep -q "Registered"; then
    echo "Registrando Microsoft.Storage..."
    az provider register --namespace Microsoft.Storage
fi

echo -e "${GREEN}✓ Resource Providers em processo de registro (continuará em background)${NC}\n"
sleep 30

# --- Criar Infraestrutura ---

echo -e "${YELLOW}Etapa 3: Criando Grupo de Recursos...${NC}"
az group create \
    --name $RESOURCE_GROUP \
    --location $LOCATION \
    --output none
echo -e "${GREEN}✓ Grupo de Recursos criado${NC}\n"

echo -e "${YELLOW}Etapa 4: Criando Servidor SQL...${NC}"
az sql server create \
    --name $DB_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $DB_ADMIN_USER \
    --admin-password $DB_ADMIN_PASSWORD \
    --output none
echo -e "${GREEN}✓ Servidor SQL criado${NC}\n"

echo -e "${YELLOW}Etapa 5: Configurando Firewall...${NC}"
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name AllowAzureServices \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 0.0.0.0 \
    --output none
az sql server firewall-rule create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name AllowAll \
    --start-ip-address 0.0.0.0 \
    --end-ip-address 255.255.255.255 \
    --output none
echo -e "${GREEN}✓ Firewall configurado${NC}\n"

echo -e "${YELLOW}Etapa 6: Criando Banco de Dados...${NC}"
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $DB_SERVER_NAME \
    --name $DB_NAME \
    --service-objective S0 \
    --backup-storage-redundancy Local \
    --output none
echo -e "${GREEN}✓ Banco de Dados criado${NC}\n"

echo -e "${YELLOW}Etapa 7: Criando App Service Plan...${NC}"
az appservice plan create \
    --name $APP_SERVICE_PLAN \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --sku B1 \
    --is-linux \
    --output none
echo -e "${GREEN}✓ App Service Plan criado${NC}\n"

echo -e "${YELLOW}Etapa 8: Criando Web App...${NC}"
az webapp create \
    --resource-group $RESOURCE_GROUP \
    --plan $APP_SERVICE_PLAN \
    --name $WEB_APP_NAME \
    --runtime "DOTNETCORE:8.0" \
    --output none
echo -e "${GREEN}✓ Web App criado${NC}\n"

echo -e "${YELLOW}Etapa 9: Configurando Connection String...${NC}"
CONNECTION_STRING="Server=tcp:${DB_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${DB_NAME};Persist Security Info=False;User ID=${DB_ADMIN_USER};Password=${DB_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

az webapp config connection-string set \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --settings DefaultConnection="$CONNECTION_STRING" \
    --connection-string-type SQLAzure \
    --output none
echo -e "${GREEN}✓ Connection String configurada${NC}\n"

echo -e "${YELLOW}Etapa 10: Configurando App Settings...${NC}"
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --settings \
        ASPNETCORE_ENVIRONMENT="Production" \
        WEBSITE_RUN_FROM_PACKAGE="0" \
        SCM_DO_BUILD_DURING_DEPLOYMENT="false" \
    --output none
echo -e "${GREEN}✓ App Settings configurado${NC}\n"

# --- Deploy da Aplicação ---

echo -e "${YELLOW}Etapa 11: Preparando projeto .NET...${NC}"

PROJECT_FILE=$(find . -maxdepth 2 -name "*.csproj" -o -name "*.sln" | head -n 1)

if [ -z "$PROJECT_FILE" ]; then
    echo "Nenhum projeto encontrado. Clonando repositório..."
    
    if [ -d "challenge-moto-connect" ]; then
        echo "Removendo clone anterior..."
        rm -rf challenge-moto-connect
    fi
    
    git clone https://github.com/mateush-souza/challenge-moto-connect.git
    cd challenge-moto-connect
    
    PROJECT_FILE=$(find . -maxdepth 2 -name "*.sln" | head -n 1)
    
    if [ -z "$PROJECT_FILE" ]; then
        echo -e "${YELLOW}Erro: Não foi possível encontrar projeto .NET no repositório clonado.${NC}"
        exit 1
    fi                                                                  
fi

echo "Projeto encontrado: $PROJECT_FILE"
echo -e "${YELLOW}Compilando aplicação...${NC}"
dotnet publish "$PROJECT_FILE" -c Release -o ./publish
echo -e "${GREEN}✓ Aplicação compilada${NC}\n"

echo -e "${YELLOW}Etapa 12: Fazendo deploy para Azure...${NC}"
rm -f app.zip
python3 - <<'PYTHON'
import os
import zipfile

with zipfile.ZipFile('app.zip', 'w', zipfile.ZIP_DEFLATED) as archive:
    for root, _, files in os.walk('publish'):
        for name in files:
            path = os.path.join(root, name)
            archive.write(path, os.path.relpath(path, 'publish'))
PYTHON

az webapp deployment source config-zip \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --src app.zip

rm -f app.zip
rm -rf ./publish

echo -e "${GREEN}✓ Deploy concluído${NC}\n"

echo -e "${YELLOW}Etapa 13: Reiniciando Web App...${NC}"
az webapp restart \
    --resource-group $RESOURCE_GROUP \
    --name $WEB_APP_NAME \
    --output none

sleep 5

echo -e "${GREEN}✓ Web App reiniciado${NC}\n"

# --- Finalização ---

WEBAPP_URL="https://$WEB_APP_NAME.azurewebsites.net"

echo -e "${GREEN}======================================================${NC}"
echo -e "${GREEN}DEPLOY CONCLUÍDO COM SUCESSO!${NC}"
echo -e "${GREEN}======================================================${NC}"
echo -e "URL da Aplicação: ${YELLOW}$WEBAPP_URL${NC}"
echo -e "Servidor SQL: ${YELLOW}${DB_SERVER_NAME}.database.windows.net${NC}"
echo -e "Banco de Dados: ${YELLOW}$DB_NAME${NC}"
echo -e "Usuário: ${YELLOW}$DB_ADMIN_USER${NC}"
echo ""
echo "Aguarde 2-3 minutos para a aplicação iniciar completamente."
echo -e "${GREEN}======================================================${NC}"
