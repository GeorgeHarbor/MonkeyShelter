#!/bin/bash
set -e
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
  -- give your user all rights on the new DBs
  GRANT ALL PRIVILEGES ON DATABASE monkeyAuth TO "$POSTGRES_USER";
  GRANT ALL PRIVILEGES ON DATABASE monkeyService  TO "$POSTGRES_USER";
  GRANT ALL PRIVILEGES ON DATABASE monkeyAudit  TO "$POSTGRES_USER";
EOSQL
