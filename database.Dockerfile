FROM postgres:12

RUN apt update -y
RUN apt install postgis postgresql-12-postgis-3 -y

COPY ./init_db/*.sql /docker-entrypoint-initdb.d/