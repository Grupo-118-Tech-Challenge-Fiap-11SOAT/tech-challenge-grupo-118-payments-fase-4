# language: pt-BR

Funcionalidade: Criação de Pagamento
    Como um sistema de pedidos
    Eu quero criar um pagamento para um pedido
    Para que o cliente possa efetuar o pagamento via QR Code

Cenário: Criar pagamento com sucesso para um novo pedido
    Dado que eu tenho um pedido com ID "ORDER-12345" e valor de 100.50
    Quando eu solicito a criação do pagamento
    Então o pagamento deve ser criado com sucesso
    E o pagamento deve conter um código QR
    E o status do pagamento deve ser "Pending"
    E o valor do pagamento deve ser 100.50
    E o pagamento deve estar persistido no banco de dados

Cenário: Criar pagamento com valor específico
    Dado que eu tenho um pedido com ID "ORDER-98765" e valor de 250.75
    Quando eu solicito a criação do pagamento
    Então o pagamento deve ser criado com sucesso
    E o pagamento deve conter um código QR
    E o valor do pagamento deve ser 250.75
    E o ID do pedido deve ser "ORDER-98765"

Cenário: Retornar pagamento existente quando pedido já possui pagamento
    Dado que já existe um pagamento para o pedido "ORDER-EXISTING"
    E que eu tenho um pedido com ID "ORDER-EXISTING"
    Quando eu solicito a criação do pagamento
    Então o pagamento existente deve ser retornado
    E o ID do pedido deve ser "ORDER-EXISTING"

