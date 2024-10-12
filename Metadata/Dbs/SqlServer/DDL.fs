namespace Metadata.Dbs.SqlServer

module DDL =

    open Metadata.Metadata
    open Microsoft.Data.SqlClient

    let private sqlColumnTypeToSqlServerType (colType: SqlType) =
        match colType with
        | Int _ -> "INT"
        | Int64 _ -> "BIGINT"
        | Float -> "FLOAT"
        | Double -> "FLOAT"
        | Decimal (precision, scale) -> sprintf "DECIMAL(%d, %d)" precision scale
        | String (Some maxLength) when maxLength <= 4000 -> sprintf "NVARCHAR(%d)" maxLength
        | String _ -> "NVARCHAR(MAX)"
        | Text -> "NVARCHAR(MAX)"
        | Bool -> "BIT"
        | DateTime -> "DATETIME2"
        | Date -> "DATE"
        | Byte -> "TINYINT"
        | ByteArray (Some maxLength) when maxLength <= 8000 -> sprintf "VARBINARY(%d)" maxLength
        | ByteArray _ -> "VARBINARY(MAX)"
        | Binary (Some maxLength) when maxLength <= 8000 -> sprintf "BINARY(%d)" maxLength
        | Binary _ -> "VARBINARY(MAX)"
        | Guid -> "UNIQUEIDENTIFIER"

    let private generateCreateTableScript (table: TableMetadata) =
        let columnsSql =
            table.Columns
            |> List.map (fun col ->
                let dataType = sqlColumnTypeToSqlServerType col.DataType
                let nullable = if col.Nullable then "NULL" else "NOT NULL"
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

        sprintf "CREATE TABLE %s (\n%s\n);" table.TableName columnsSql

    open Metadata.Env

    let createTables (dbMetadata: Metadata) =
        let connString = getConnectionString dbMetadata.connectionStringEnvKey
        use conn = new SqlConnection(connString)
        conn.Open()

        match dbMetadata.TypesToTables with
        | Some typesToTables ->
            typesToTables
            |> Map.iter (fun _ table ->
                let script = generateCreateTableScript table
                use cmd = new SqlCommand(script, conn)
                cmd.ExecuteNonQuery() |> ignore
            )
        | None -> printfn "No table metadata available."
        
        conn.Close()