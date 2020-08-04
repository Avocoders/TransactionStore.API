CREATE procedure Transaction_AddTransfer
	@leadId bigint,
	@typeId tinyint,
	@currencyId tinyint,
	@amount money,
	@leadIdReceiver bigint
as
begin
	declare @timestamp datetime2 = sysdatetime()
	insert into [dbo].[Transaction] (LeadId, TypeId, CurrencyId, Amount, [Timestamp])
		values (@leadId, @typeId, @currencyId, -@amount, @timestamp)
		declare @lead bigint set @lead = scope_identity()
	insert into [dbo].[Transaction] (LeadId, TypeId, CurrencyId, Amount, [Timestamp])
		values (@leadIdReceiver, @typeId, @currencyId, @amount, @timestamp)
		select @lead as [lead]
		union select scope_identity()
end