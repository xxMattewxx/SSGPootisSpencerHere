## SSG

```shell
./SSGDistributer --port <portId>
```

# Build image

```shell
docker-compose up --build --force-recreate
```

# Restore SQL

```shell
gdown https://drive.google.com/uc?id=<id>
unzip bd.zip
sed -i 's/`//g' bd.sql
sh run.sh
tail -n +37 bd.sql | docker exec -i SSG_db psql -U ssg -d ssg
```