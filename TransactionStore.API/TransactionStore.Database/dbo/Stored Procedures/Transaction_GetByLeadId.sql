CREATE procedure [dbo].[Transaction_GetByLeadId]
  @leadId bigint
as
begin
  select two.LeadId as TransientLeadId, one.Id, one.LeadId, one.Amount, one.CurrencyId, one.TypeId, one.[Timestamp]
	from [dbo].[Transaction] as one
		full outer join [dbo].[Transaction] as two on one.CurrencyId=two.CurrencyId and one.TypeId = two.TypeId
			and one.[Timestamp]=two.[Timestamp] and one.Amount<>two.Amount and abs(one.Amount)=abs(two.Amount)
	where one.LeadId=@leadId
end