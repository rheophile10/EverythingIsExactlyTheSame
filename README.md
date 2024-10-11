<img src="README.png" alt=":|" style="display: block; margin: auto;" />

This was a silly experiment to explor DB data access Fsharp. I wanted something that was like .sqlproj but didn't use EF Core or any OOP. Of course I could not do that because all the clients I used were ADO clients. It was a futile effort. I also wasn't using those tools effectively.

Fsharp is built for metadata management and EFCore is an OOP abomination. The whole benefit of Fsharp is domain driven design.

But when you look at how easy it is to use EF Core with Fsharp it's probably irresponsible to avoid it.

With EF Core its really easy to make Fsharp types and integrate them.

```
open System
open System.ComponentModel.DataAnnotations
open System.ComponentModel.DataAnnotations.Schema

[<CLIMutable>]
type Customer = {
    [<Key>]
    Id: int
    Name: string
    Email: string
}

open Microsoft.EntityFrameworkCore
open MyFSharpDataAccess.Models

type DbContextName(options: DbContextOptions<DbContextName>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable customers: DbSet<Customer>
    member this.Customers with get() = this.customers and set v = this.customers <- v

    override _.OnModelCreating(modelBuilder: ModelBuilder) =
        base.OnModelCreating(modelBuilder)

```

But I don't like that I have to use an EF Core CLI tool to do migrations and things. I don't like the idea of doing data model versioning outside of git either.

Metadata is important.
