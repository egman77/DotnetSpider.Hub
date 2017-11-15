if [ -n "$1" ]; then
		if [ -f "/opt/DotnetSpider.Agent/agent.lock" ]; then
			echo "agent is ruuning."
		else
			rm /opt/DotnetSpider.Agent -rf
			packageUrl="http://nasabigdata.com:30012/contents/dotnetspider.enterprise/$1"
			echo $packageUrl
			rm /tmp/$1 -f
			echo "remove /tmp/$1 success"
			wget http://nasabigdata.com:30012/contents/dotnetspider.enterprise/$1  -O /tmp/$1
			echo "download http://nasabigdata.com:30012/contents/dotnetspider.enterprise/$1 success"
			unzip -o /tmp/$1  -d /opt
			rm /opt/DotnetSpider.Agent/config.ini -f
			wget http://nasabigdata.com:30012/contents/dotnetspider.enterprise/config.ini -O /opt/DotnetSpider.Agent/config.ini
			cd /opt/DotnetSpider.Agent
			nohup dotnet DotnetSpider.Enterprise.Agent.dll --daemon &
		fi
else
echo "version missing."
fi