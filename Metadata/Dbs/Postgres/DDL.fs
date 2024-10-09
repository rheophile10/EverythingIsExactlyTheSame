namespace Metadata.Dbs.Postgres

module DDL =

    open Metadata.Metadata
    open Metadata.Env

    let private sqlColumnTypeToPostgresType (colType: SqlColumnType) =
        match colType with
        | Int _ -> "INTEGER"
        | String  (Some maxLength) -> sprintf "VARCHAR(%d)" maxLength
        | String None -> "TEXT"
        | Bool -> "BOOLEAN"
        | Decimal (precision, scale) -> sprintf "DECIMAL(%d, %d)" precision scale
        | Float -> "REAL"
        | Double -> "DOUBLE PRECISION"
        | DateTime -> "TIMESTAMP"
        | Date -> "DATE"
        | Byte -> "SMALLINT"
        | ByteArray _ -> "BYTEA"
        | Binary _ -> "BYTEA"
        | Text -> "TEXT"

    let private generateCreateTableScript (table: TableMetadata) =
        let columnsSql =
            table.Columns
            |> List.map (fun col ->
                let dataType = sqlColumnTypeToPostgresType col.DataType
                let nullable = if col.Nullable then "" else "NOT NULL"
                let constraints =
                    col.Constraints
                    |> List.map (function
                        | PrimaryKey -> "PRIMARY KEY"
                        | Unique -> "UNIQUE"
                        | ForeignKey (refTable, refColumn) ->
                            sprintf "REFERENCES %s(%s)" refTable refColumn
                    )
                    |> String.concat " "
                sprintf "%s %s %s %s" col.Name dataType nullable constraints
            )
            |> String.concat ",\n"

        sprintf "CREATE TABLE IF NOT EXISTS %s (\n%s\n);" table.TableName columnsSql

    open Npgsql.FSharp

    let createTables (dbMetadata: Metadata) =
        let connString = getConnectionString dbMetadata.connectionStringEnvKey
        let connection = Sql.connect connString

        match dbMetadata.TypesToTables with
        | Some typesToTables ->
            typesToTables
            |> Map.iter (fun _ table ->
                let script = generateCreateTableScript table
                connection
                |> Sql.query script
                |> Sql.executeNonQuery
                |> ignore
            )
        | None ->
            printfn "No table metadata available."
