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
docker exec -it SSG_db psql -U ssg
CREATE TABLE seeds (
  id bigint NOT NULL PRIMARY KEY,
  task_id bigint DEFAULT NULL,
  structure_seed bigint NOT NULL,
  chunk_x integer NOT NULL,
  chunk_z integer NOT NULL,
  mc_version integer DEFAULT NULL
);
CREATE INDEX seed_select_idx ON seeds (structure_seed, mc_version);
CREATE INDEX task_id_idx ON seeds (task_id);
tail -n +37 bd.sql | docker exec -i SSG_db psql -U ssg -d ssg
```