# RabbitMQ

#Take a look on Connectionstring if you cannot run update-database

add-migration init-db
update-database

#Run docker

docker pull rabbitmq:3-management

docker run --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:3-management

