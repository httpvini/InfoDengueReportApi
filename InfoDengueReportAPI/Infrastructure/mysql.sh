#!/usr/bin/env bash

if [ $1 == "clear" ]
then
  echo "Limpando containers"
  docker rm -f $(docker ps -a -q)
fi

echo "Baixando imagem"
docker pull mysql:8.0.19

echo "Criando container"
docker run --name eventoapp -e MYSQL_ROOT_PASSWORD=root -p 3307:3306 -d mysql:8.0.19

echo "Aguardando o banco subir"
sleep 31

echo "Digite a senha do banco"
docker exec -it eventoapp mysql -p