FatureJa
========

Exemplo de processamento paralelo com Windows Azure.

Autor: Fernando Correia (@facorreia)


Propósito
---------

Demonstrar uma arquitetura para processamento distribuído e paralelo na plataforma Windows Azure.

Em particular, são demonstrados estes padrões:

* Separação da camada web da camada de processos de negócio.
* Particionamento de comandos em mensagens.
* Processamento assíncrono, distribuído e paralelo de mensagens.
* Monitoramento de progresso.
* Registro de informações para diagnóstico.


Limitações
----------

Este projeto tem objetivos puramente didáticos.

Muitos requisitos necessários para uma aplicação em produção não foram implementados.

Particularmente, apenas as otimizações mais fundamentais de performance foram feitas.

O tratamento de erros também se atém ao básico.


Ferramentas e bibliotecas utilizadas
------------------------------------

* Visual Studio 2010
* ASP.NET MVC 4 RC
* Windows Azure SDK for .NET 1.6
