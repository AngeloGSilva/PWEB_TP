# PWEB_TP

[Administrador] Área de administrador da plataforma
### Gestão de empresas
- [ ] Listar registo de empresas - com filtros (nome, estado da subscrição) e com ordenação
- [ ] Adicionar registo de empresas - automaticamente cria um utilizador com perfil gestor, associado à empresa
- [ ] Editar registo de empresas
- [ ] Apagar registo de empresas (apenas se ainda não existirem veículos)
- [ ] Ativar ou inativar empresas

### Gestão de categorias de veículos
- [ ] Listar
- [ ] Criar
- [ ] Editar
- [ ] Apagar registos

### Gestão de utilizadores
- [ ] Listar
- [ ] Editar
- [ ] Ativar ou inativar utilizadores

[Gestor] Área de gestor de uma empresa de aluguer
### Gestão de funcionários
- [ ] Criar registo de utilizadores, com o perfil funcionário, associado à sua empresa
- [ ] Criar registo de utilizadores, com o perfil gestor, associado à sua empresa
- [ ] Listar registo de utilizadores
- [ ] Ativar ou inativar registo de utilizadores
- [ ] Apagar registo de utilizador (caso não esteja associado a nenhuma reserva)
- [ ] Não pode apagar nem desativar o seu próprio registo de utilizador

### Gestão de reservas
- [ ] Igual ao perfil Funcionário

### Gestão da frota de veículos 
- [ ] Igual ao perfil Funcionário

[Funcionário] Área de funcionário de uma empresa de aluguer
### Gerir a frota de veículos da empresa

- [ ] Listar registo de veículos - com filtros (categoria, estado) e com ordenação
- [ ] Adicionar registo de veículos
- [ ] Editar registo de veículos
- [ ] Apagar registo de veículos (apenas se já não existirem reservas desse veículo)
- [ ] Ativar ou inativar registo de veículos

### Gerir as reservas da empresa

- [ ] Listar reservas - com filtros (data de levantamento, data entrega, categoria, veículo, cliente)
- [ ] Confirmar uma reserva
- [ ] Rejeitar uma reserva
- [ ] Entregar um veículo ao cliente (levantamento do veículo pelo cliente) 
- [ ] Receber um veículo do cliente (entrega do veículo pelo cliente)
_Assinalar o estado em que o veículo é entregue/levantado ao/pelo cliente_


[Cliente] Área de clientes
- [ ] Efectuar uma pesquisa (similar ao utilizador anónimo)
- [ ] Efectuar a reserva de um veículo
- [ ] Consultar o histórico de reservas
- [ ] Consultar os detalhes de uma reserva

[Utilizador Anónimo] Área de acesso livre
- [ ] Pesquisar veículos por localização, tipo de veículo e data (data de levantamento e data de entrega)
- [ ] O resultado da pesquisa deve ser uma lista dos veículos disponíveis, indicando o custo e a empresa de aluguer a que pertence, bem como a avaliação da empresa
- [ ] Deve ser possível filtrar os resultados por categoria de veículo e por empresa
- [ ] Deve ser possível ordenar os resultados por preço mais baixo/alto e/ou por classificação da empresa
- [ ] Efectuar o registo como Cliente
