# RabbitMqWorkerExperiment
Project to learn about RabbitMq. There are two console applications, one simulating a Producer  and other a Consumer. 

## RabbitMq docker command
docker run -d --hostname rabbitserver --name rabbitmq-server -p 15672:15672 -p 5672:5672 -e RABBITMQ_DEFAULT_USER=myuser -e RABBITMQ_DEFAULT_PASS=mypassword rabbitmq:3-management

Open the project and run `OrderConsumer`. After that, run `OrderProducer`. It will start to produce messages to queue and the consumer will consume it. If `OrderProducer` produces more than `OrderConsumer` can handle, you can run other instance of `OrderConsumer` to handle all messages.
