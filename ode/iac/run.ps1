# az login --tenant c8f6ed62-b179-45b2-9867-83de02085daa

$rgName = 'ODE_OpenHack_042022'

az group create --location 'west europe' --name $rgName

$deployment = "$rgName_$([string]::Format("{0:yyyyMdd-HHmmss}", [datetime]::Now))"

az deployment group create --resource-group $rgName --name "$deployment" --template-file infra.bicep