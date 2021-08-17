import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-squareobject',
    templateUrl: './so.component.html',
    styleUrls: ['./so.component.css']
})
export class SquareObjectComponent {
    public squareobjectlist: ProtoSquareObject[];
    public runmodes: string[];
    public selectedRunmode:string;
    public selectedObjectLevel:number;
    public activeRunmode:string;
    private baseUrl: string;
    public myid: string;
    public currentTime: Date;

    public injectionPower:number = 100;
    public injectionBox:number = 100;

    public currentObject: ProtoSquareObject;
    public isDataLoading: boolean;
    
    
    constructor(
        private datepipe: DatePipe,
        private route: ActivatedRoute,
        private http: HttpClient,
        @Inject('BASE_URL') baseUrl: string) {
        this.baseUrl = baseUrl;

        this.route.queryParams.subscribe(params => {
            this.selectedObjectLevel = 1;
            this.myid = params['id'];
            this.getList();
            this.runmodeList();
        });
    }

    ngOnInit(){
        setInterval(() => {
            this.currentTime = new Date();
        },1000);
    }
       

    //API
    public changeRunMode()
    {
        this.isDataLoading = true;
        this.http.get<string>(this.baseUrl + 'api/userinfo/select/' + this.myid+'?runmodeString='+this.selectedRunmode, { responseType: "text" as 'json' }).subscribe(result => {
            
            if (result == 'Error')
            {
                alert(result);
                return;   
            }
            this.selectedRunmode = result;
            this.activeRunmode = result;
            alert('Success');
            this.isDataLoading = false;
        }, error => console.error(error));
    }

    public getMode(){
        this.selectedRunmode = null;
        this.http.get<string>(this.baseUrl + 'api/userinfo/getmode/' + this.myid, { responseType: "text" as 'json' }).subscribe(result => {
            this.selectedRunmode = result;
            this.activeRunmode = result;
        }, error => console.error(error));
    }

    public runmodeList() {
        this.runmodes = null;
        this.http.get<string[]>(this.baseUrl + 'api/userinfo/runmode').subscribe(result => {
            this.runmodes = result;
            this.getMode();
        }, error => console.error(error));
    }

    public getList() {
        this.squareobjectlist = null;
        this.http.get<ProtoSquareObject[]>(this.baseUrl + 'sop/' + this.myid + '/list').subscribe(result => {
            
            this.squareobjectlist = result;
        }, error => console.error(error));
    }

    public newSo() {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.myid + '/new').subscribe(result => {
            this.squareobjectlist.push(result);
        }, error => console.error(error));
    }

    public getSo(id:number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/so').subscribe(result => {
            this.replaceFromCurrentSO(result);
        }, error => console.error(error));
    }

    public removeSo(){
        var index = this.squareobjectlist.findIndex((obj)=>obj.id == this.currentObject.id);
        if (index < 0)
        {
            alert('잘못된 객체');
            return;   
        }    
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.currentObject.id + '/del').subscribe(result => {
           
            this.squareobjectlist.splice(index,1);
            this.currentObject = null;
        }, error => console.error(error));
    }


    //API behaviors
    public startSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/start/'+ (this.selectedObjectLevel + 1)).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));
    }

    public restartSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/restart').subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    

    public getreward(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/reward').subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public levelup(type:number){
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.currentObject.id + '/levelup/' + type).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public leveldown(type:number){
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.currentObject.id + '/leveldown/'+type).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public forcelevelup(type:number){
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.currentObject.id + '/forcelevelup/' + type).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public forceleveldown(type:number){
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + this.currentObject.id + '/forceleveldown/'+type).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }


    public stopSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/stop').subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public powerSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/power/' + this.injectionPower).subscribe(result => {
            if (!result)
                alert('에너지가 부족');
            else
                this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public boxSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/box/' + this.injectionBox).subscribe(result => {
            if (!result)
                alert('에너지가 부족');
            else
                this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public donateSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/donate').subscribe(result => {
            this.replaceFromCurrentSO(result);            
        }, error => console.error(error));

    }

    public helpSo(id: number) {
        this.http.get<ProtoSquareObject>(this.baseUrl + 'sop/' + id + '/help').subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public onChangePremium()
    {
        this.currentObject.isPremium = !this.currentObject.isPremium;
        this.http.post<ProtoSquareObject>(this.baseUrl + 'sop/update',this.currentObject).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));

    }

    public addTime(addmin:number)
    {
       this.currentObject.addTimeSpan += addmin;
        this.http.post<ProtoSquareObject>(this.baseUrl + 'sop/update',this.currentObject).subscribe(result => {
            this.replaceFromCurrentSO(result);     
        }, error => console.error(error));
    }

    //utility
    public toTime(date:Date): string  {
        return this.datepipe.transform(new Date(date),'MM/dd HH:mm')
    }

    private replaceFromCurrentSO(newSO: ProtoSquareObject)
    {
        if (newSO)
        {
            this.currentObject = newSO;
            var index = this.squareobjectlist.findIndex((obj)=>obj.id == this.currentObject.id);
            if (index >= 0)
            {
                this.squareobjectlist[index] = this.currentObject;
            } 
        }
    }

    public disableEnergy():boolean {
        return 100 > this.currentObject.currentState.coreEnergy + this.currentObject.currentState.extraCoreEnergy
    }


    public toTimeSpan():Date {
        var m = this.currentObject.addTimeSpan % 60;
        var h = (this.currentObject.addTimeSpan / 60) % 24;
        var d = (this.currentObject.addTimeSpan / ( 60 * 24));

        var rDate = new Date(Date.now() + this.currentObject.addTimeSpan * 60000);
        return rDate;
    }

}


interface ProtoSquareObject
{
    id: number;
    donate: number;
    help: number;
    isPremium:boolean;
    addTimeSpan:number;
    donationTime:Date;
    helpTime:Date;
    totalDonate:number;
    totalHelp:number;
    usedTicket:number;

    energyCycle:number;
    energyQuantity:number;
    powerCycle:number;
    powerQuantity:number;
    invasionHistory:ProtoSquareObjectInvasionHistory[];



    squareObjectExp: number;
    squareObjectLevel: number;
    coreExp: number;
    coreLevel: number;
    agencyExp: number;
    agencyLevel: number;

    currentState: ProtoSquareObjectState;

}

interface ProtoSquareObjectState
{
    userId:number;
    isActivated:boolean;
    nextInvasionLevel:number;
    activatedTime:Date;
    nextInvasionTime:Date;
    nextInvasionMonsterId:number;
    squareObjectLevel:number;
    currentPlanetBoxExp:number;
    currentPlanetBoxLevel:number;
    currentShield:number;
    squareObjectPower:number;
    powerRefreshTime:Date;
    coreEnergy:number;
    extraCoreEnergy:number;
    energyRefreshTime:Date;
    extraEnergyInjectedTime:Date;
    enableReward:boolean;
    invasionHistory:ProtoSquareObjectInvasionHistory[];
    
}

interface ProtoSquareObjectInvasionHistory
{
    monsterId:number;
    invasionLevel:number;
    invasionTime:Date;
    remainedShield:number;
    power:number;
    monsterAtk:number;
    previousShield:number;
    gettingCoreExp:number;
    gettingAgencyExp:number;
    monsterCount:number;
}
