/KServer/Scripts/Common/kill_gmtool.sh

cd /KServer/GMTool
pip3 install -r requirements.txt
python36 manage.py makemigrations
python36 manage.py migrate
python36 manage.py runserver 0.0.0.0:63545 --settings GMTool.settings >log.out 2>log.err &
