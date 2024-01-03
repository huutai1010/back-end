#!/bin/bash

db_name=etraveldb
wait_time=60s
password=Abcd@1234

# wait for SQL Server to come up
echo importing data will start in $wait_time...
sleep $wait_time
echo importing data...

chmod 701 ./sample_db.sql

# run the init script to create the DB
RESULT=$(/opt/mssql-tools/bin/sqlcmd -S "host.docker.internal,1107" -U "sa" -P "$password" -d master -W -h-1 -k -q "SET NOCOUNT ON; SELECT count(*) FROM sys.databases WHERE name = '$db_name'")
if [[ $RESULT -eq 1 ]] 
then
    echo "Database exists. Skip create"
else 
    echo "Database not exists. Creating ..."
    /opt/mssql-tools/bin/sqlcmd -S "host.docker.internal,1107" -U "sa" -P "$password" -i ./sample_db.sql
fi