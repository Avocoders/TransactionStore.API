CREATE procedure [dbo].[Transaction_Add]
	@leadId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount money	
as
begin
	declare @timestamp datetime2 = sysdatetime()
	insert into [dbo].[Transaction](LeadId, TypeId, CurrencyId, Amount, [Timestamp]) values(@leadId, @typeId, @currencyId, @amount, @timestamp)
	select scope_identity();
end