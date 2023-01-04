# PWEB_TP

[Administrador] Área de administrador da plataforma
### Gestão de empresas
- [1/2] Listar registo de empresas - com filtros (nome, estado da subscrição) e com ordenação
- [x] Adicionar registo de empresas - automaticamente cria um utilizador com perfil gestor, associado à empresa
- [x] Editar registo de empresas
- [1/2] Apagar registo de empresas (apenas se ainda não existirem veículos)
- [1/2] Ativar ou inativar empresas

### Gestão de categorias de veículos
- [x] Listar
- [x] Criar
- [x] Editar
- [x] Apagar registos

### Gestão de utilizadores
- [x] Listar
- [x] Editar
- [x] Ativar ou inativar utilizadores

[Gestor] Área de gestor de uma empresa de aluguer
### Gestão de funcionários
- [x] Criar registo de utilizadores, com o perfil funcionário, associado à sua empresa
- [x] Criar registo de utilizadores, com o perfil gestor, associado à sua empresa
- [x] Listar registo de utilizadores
- [x] Ativar ou inativar registo de utilizadores
- [] Apagar registo de utilizador (caso não esteja associado a nenhuma reserva)
- [x] Não pode apagar nem desativar o seu próprio registo de utilizador

### Gestão de reservas
- [X] Igual ao perfil Funcionário

### Gestão da frota de veículos 
- [x] Igual ao perfil Funcionário

[Funcionário] Área de funcionário de uma empresa de aluguer
### Gerir a frota de veículos da empresa

- [1/2] Listar registo de veículos - com filtros (categoria, estado) e com ordenação //falta filtrar por categoria?
- [x] Adicionar registo de veículos
- [1/2] Editar registo de veículos -> todos os parametros podem ser alterados à exceção da categoria que não está a funcionar. Esconder ID da empresa pq nao pode ser alterado obviamente.
- [1/2] Apagar registo de veículos (apenas se já não existirem reservas desse veículo)
- [x] Ativar ou inativar registo de veículos

### Gerir as reservas da empresa

- [1/2] Listar reservas - com filtros (data de levantamento, data entrega, categoria, veículo, cliente)
- [X] Confirmar uma reserva
- [ ] Rejeitar uma reserva
- [X] Entregar um veículo ao cliente (levantamento do veículo pelo cliente) 
- [X] Receber um veículo do cliente (entrega do veículo pelo cliente)
- [x]Assinalar o estado em que o veículo é entregue/levantado ao/pelo cliente_


[Cliente] Área de clientes
- [X] Efectuar uma pesquisa (similar ao utilizador anónimo)
- [X] Efectuar a reserva de um veículo
- [X] Consultar o histórico de reservas
- [X] Consultar os detalhes de uma reserva

[Utilizador Anónimo] Área de acesso livre
- [X] Pesquisar veículos por localização, tipo de veículo e data (data de levantamento e data de entrega)
- [1/2] O resultado da pesquisa deve ser uma lista dos veículos disponíveis, indicando o custo e a empresa de aluguer a que pertence, bem como a avaliação da empresa
- [ ] Deve ser possível filtrar os resultados por categoria de veículo e por empresa
- [ ] Deve ser possível ordenar os resultados por preço mais baixo/alto e/ou por classificação da empresa
- [X] Efectuar o registo como Cliente
