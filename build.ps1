$today =  Get-Date -Format "yy.MM.dd"
$today = "0.1." + $today

$buildTag = "bbprimemgr:" + $today
$ecrTag = "591069284328.dkr.ecr.us-west-1.amazonaws.com/bk-bbprime-mgr:" + $today

echo "Local build tag is: $buildTag"
echo "ECR tag is: $ecrTag"

docker build -t  $buildTag -f .\PrimeCollaborationManager\Dockerfile .
docker tag $buildTag $ecrTag

echo "Done building and tagging."