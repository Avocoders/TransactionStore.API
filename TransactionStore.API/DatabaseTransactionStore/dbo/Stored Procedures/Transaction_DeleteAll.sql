CREATE PROCEDURE [dbo].[Transaction_DeleteAll]
AS
begin
	truncate table [Transaction]
end
