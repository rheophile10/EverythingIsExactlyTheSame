namespace Metadata

module Metadata =

    type Constraint =
        | PrimaryKey
        | ForeignKey of table: string * column: string
        | Unique

    type SqlColumnType =
        | Int of scale: int option
        | String of maxLength: int option
        | Bool
        | Decimal of precision: int * scale: int
        | Float
        | Double
        | DateTime
        | Date
        | Byte
        | ByteArray of maxLength: int option
        | Binary of maxLength: int option
        | Text

    type ColumnMetadata = {
        Name: string
        DataType: SqlColumnType
        Nullable: bool
        OrdinalPosition: int
        Length: int option
        DefaultValue: obj option
        Constraints: Constraint list
    }

    type TableMetadata = {
        TableName: string
        Columns: ColumnMetadata list
    }

    type Database = 
        | SQLServer
        | Postgres
        | SQLite

    open System

// let myType = typeof<MyType>
// let typeName = myType.FullName

    type Metadata = {
        connectionStringEnvKey: string        
        TypesToTables: Map<string, TableMetadata> option
        database: Database
    }