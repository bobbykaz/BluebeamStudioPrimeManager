$today =  Get-Date -Format "yy.MM.dd"
$today = "0.1." + $today

$ecrTag = "591069284328.dkr.ecr.us-west-1.amazonaws.com/bk-bbprime-mgr:" + $today

aws ecr get-login-password --region us-west-1 | docker login --username AWS --password-stdin 591069284328.dkr.ecr.us-west-1.amazonaws.com

docker push $ecrTag