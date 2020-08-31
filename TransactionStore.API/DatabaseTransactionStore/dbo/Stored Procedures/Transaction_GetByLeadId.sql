CREATE procedure [dbo].[Transaction_GetByLeadId]
  @leadId bigint
as
begin
    select 
       id,
       LeadId,
       Amount,
       [Timestamp],
       typeId,
       currencyId
     into #SearchResult 
     from dbo.[Transaction] 
     where (LeadId=@leadId )
    
     select 
         t.Id,
         t.LeadId,
         t.Amount,
         t.[Timestamp],
         t.typeId as id,
         t.currencyId as id
      from #SearchResult s
      join [Transaction] t on s.CurrencyId=t.CurrencyId and 
                         s.TypeId = t.TypeId and
                         s.[Timestamp]=t.[Timestamp] and
                         s.Amount<>t.Amount and
                         abs(s.Amount)=abs(t.Amount)
      union select 
           Id,
           LeadId,
           Amount,
           [Timestamp],
           typeId,
           currencyId
       from #SearchResult 
end