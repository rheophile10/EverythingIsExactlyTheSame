<img src="README.png" alt=":|" style="display: block; margin: auto;" />

This is an inappropriate experiment to explore DB data access Fsharp. I wanted something that was like the big ddl.sql files of .sqlproj but didn't use any OOP EF Core.

look what they make us do

```
open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

[<CLIMutable>] \\ gross tags on an #f type explicitly even mentioning mutability
type Customer = {
    [<Key>] \\ another hideous tag
    Id: int
    Name: string
    Email: string
}

open Microsoft.EntityFrameworkCore \\ unnecessary abstraction
open MyFSharpDataAccess.Models

type DbContextName(options: DbContextOptions<DbContextName>) =
    inherit DbContext(options) \\ disgusting

    [<DefaultValue>] \\ again ew
    val mutable customers: DbSet<Customer>
    member this.Customers with get() = this.customers and set v = this.customers <- v

    override _.OnModelCreating(modelBuilder: ModelBuilder) =
        base.OnModelCreating(modelBuilder) \\ override ew

```

I want metadata without oop

<img src="metadata.png" alt="just a man and his pets appreciating the metadata" style="display: block; margin: auto;" />
