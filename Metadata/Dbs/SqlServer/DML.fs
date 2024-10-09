namespace Metadata.Dbs.SqlServer

module DML =

    open System.Data
    open Microsoft.Data.SqlClient
    open Metadata.Metadata

    let bulkLoadData
        (connString: string)
        (table: TableMetadata)
        (reader: IDataReader) =
        
        use bulkCopy = new SqlBulkCopy(connString, SqlBulkCopyOptions.TableLock)
        bulkCopy.DestinationTableName <- table.TableName
        table.Columns
        |> List.iter (fun col -> bulkCopy.ColumnMappings.Add(col.Name, col.Name) |> ignore)

        bulkCopy.WriteToServer(reader)
        reader.Close()

    let executeNonQuery (connString: string) (query: string) =
        use conn = new SqlConnection(connString)
        conn.Open()

        use cmd = new SqlCommand(query, conn)
        cmd.ExecuteNonQuery() |> ignore
        conn.Close()