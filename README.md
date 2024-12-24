
  # Generic Repository


A Generic Repository is a design pattern that provides a centralized and reusable approach to data access. It encapsulates the logic for common database operations (such as Create, Read, Update, Delete) in a single class or interface, making it reusable for multiple entities in an application.

The pattern is often used in conjunction with the Repository Pattern to abstract data access logic, improve code reusability, and enforce separation of concerns.


  ## Key Features of the Generic Repository


### 1.Generic Type Parameter (<T>):
The repository is designed to handle operations for any entity type.
This is achieved using a generic type parameter (T), where T is constrained to be a class (or entity) that maps to a database table.

### 2.Common CRUD Operations:
Methods like Add, Update, Delete, Get, and GetAll are implemented in a generic way.
These operations can then be reused for different entities.

### 3.Abstraction:
Abstracts data access code, so the business logic layer does not directly interact with the database.

### 4.Reusable:
Once implemented, the repository can be used for multiple entities, reducing redundant code.

#                                   CQRS
The CQRS (Command Query Responsibility Segregation) pattern is a design pattern used in software architecture to separate the responsibility of reading data (queries) from the responsibility of writing data (commands). This separation allows each operation to be optimized independently and provides greater scalability, maintainability, and clarity in complex systems.

### Key Concepts
Commands:

Used to perform write operations (e.g., create, update, delete).
Modify the state of the system.
Do not return data beyond a simple success or failure response.
