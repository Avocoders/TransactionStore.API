CREATE procedure [dbo].[TotalAmount]
	@leadId bigint
as
begin
	select sum(Amount) from [dbo].[Transaction]
	where LeadId = @leadId
end