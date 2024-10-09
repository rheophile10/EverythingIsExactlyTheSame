namespace Metadata.Dbs.Postgres

module DML =

    open Npgsql
    open Npgsql.FSharp
    open System
    open System.Data
    open Metadata.Metadata

    let bulkLoadData
        (connString: string)
        (table: TableMetadata) 
        (reader: IDataReader) =
        use conn = new NpgsqlConnection(connString)
        conn.Open()

        let columnNames = table.Columns |> List.map (fun col -> col.Name) |> String.concat ", "
        let copyCommand = sprintf "COPY %s (%s) FROM STDIN (FORMAT BINARY)" table.TableName columnNames

        use writer = conn.BeginBinaryImport(copyCommand)

        while reader.Read() do
            writer.StartRow()
            for col in table.Columns do
                let value = reader.[col.Name]
                if value = DBNull.Value then
                    writer.WriteNull()
                else
                    writer.Write(value)

        writer.Complete() |> ignore
        conn.Close()

    let executeNonQuery (connString: string) (query: string) =
        use conn = new NpgsqlConnection(connString)
        conn.Open()

        use cmd = new NpgsqlCommand(query, conn)
        cmd.ExecuteNonQuery() |> ignore
        conn.Close()