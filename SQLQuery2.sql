select * from Observations where SurveyIdentifier = '6a6075ec-014b-457a-b887-cd0c08d020a6'
delete from Observations where SurveyIdentifier = '6a6075ec-014b-457a-b887-cd0c08d020a6'

update Observations set Bin3= Bin1
where SurveyIdentifier = '6a6075ec-014b-457a-b887-cd0c08d020a6'


select * from dbo.Users

update dbo.SurveyCompleted set SubmittedBy = 1