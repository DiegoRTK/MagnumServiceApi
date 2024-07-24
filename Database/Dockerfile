FROM mcr.microsoft.com/mssql/server:2019-latest

# Copiar certificados
COPY sqlserver.crt /var/opt/mssql/
COPY sqlserver.key /var/opt/mssql/

# Copiar y ejecutar script de inicialización
COPY init.sql /var/opt/mssql/
RUN /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P Psplus12 -i /var/opt/mssql/init.sql

# Configuración adicional si es necesario
