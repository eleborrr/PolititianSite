Принцип работы.

Логин и регистрация интуитивно.

У сайта есть две штуки. Дебаты и новости.
Дебаты видят все пользователи всегда, там можно оставлять комментарии, спорить оставлять свое мнение.

Зареганный юзер видит новости только тех людей, на которых он подписан.
Незареганный видит все новости.

Подписаться на пользователя можно зайдя к нему в аккаунт и тыкнув на соовтетстующую кнопку.

Есть возможность редактировать информацию по своему аккаунту. Постить дебаты. Постить новости. Искать в поиске пользователей.

classDiagram

    class Accounts {
        Id : int, PK
        Name : nvarchar(max)
        Surname : nvarchar(max)
        About : nvarchar(max)
        Organization : nvarchar(max)
        Email : varchar(max)
    }
    
    class AccountToAccount{
      Id : int, PK
      Subscriber_id: int, FK
      Reciever_id: int, FK
    }
    
    class Comments {
        Id : int, PK
        AuthorId : int, FK
        NewsId : int, FK
        Content : nvarchar(max)
        Date : datetime2(7)
    }
    
    class Debates {
        Id : int, PK
        AuthorId : int, FK
        CreationDate : datetime2(7)
        Content : nvarchar(max)
        Title: nvarchar(max)
    }
    
    class Messages {
        Id : int, PK
        AuthorId : int, FK
        DebateId : int, FK
        Content : nvarchar(max)
        Date : datetime2(7)
    }
    
    class News {
        Id : int, PK
        Title: nvarchar(max)
        Content : nvarchar(max)
        AuthorId : int, FK
        Date : datetime2(7)
    }

    AccountToAccount ..> Accounts : Reciever_id
    AccountToAccount ..> Accounts : Subsriber_id
    Comments ..> News : NewsId
    Messages ..> Debates : DebateId
    Debates ..> Accounts : AuthorId
    News ..> Accounts : AuthorId
