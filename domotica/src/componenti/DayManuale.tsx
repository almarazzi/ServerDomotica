import { useEffect, useState } from "react";
import { DebounceInput } from "react-debounce-input";

interface Oragiorno{
  readonly oraInizio: string;
  readonly oraFine: string;
  readonly day: number;
}

export const regex = new RegExp("^((2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9]))$"); //,(([0-9][0-9][0-9][0-9])-(0[0-9]|1[0-2])-(0?[1-9]|[12][0-9]|3[0-2]))
export function DayManuale(props: {dayOfWeek: number,array: Array<Oragiorno>, mac: string}) {
  const [dataInz,SetdataInz] = useState("");
  const [dataFin,SetdataFin] = useState("");
  const [oraFin,SetoraFin] = useState("");
  const [oraInz,SetoraInz] = useState("");
  const [oraTotFin,SetoraTotFin] = useState(0);
  const [oraTotInz,SetoraTotInz] = useState(0);
  const [oraCors,SetoraCors] = useState(0);
  const [tempoCon,SeTtempoCon] = useState(0);
  const [focus, setFocus] = useState(false);
  const days = ['Domenica', 'Lunedì', 'Martedì', 'Mercoledì', 'Giovedì', 'Venerdì', 'Sabato'];
  //let d = new Date();
  const dataInizio = (t:string) =>{
    if(regex.exec(t))
    {
      SetdataInz(t);
    }
  }
  const dataFine = (t:string) =>{
    if(regex.exec(t)) 
    {
      SetdataFin(t);
    }
  }
useEffect(()=>{
  const api = async()=>{
      const x = {inizio:dataInz, fine:dataFin ,day:props.dayOfWeek, mac:props.mac }; 
      await fetch("/api/RelaySwitch/SetData", { method:"PUT", body: JSON.stringify(x), headers: { 'Content-type': 'application/json; charl set=UTF-8' }});
  }
  if((dataInz !=="" || dataFin!=="") && focus){
  api();
  }
},[focus,dataFin,dataInz]);




useEffect(()=>{

props.array.forEach((u,_)=>{
    if(u.day === props.dayOfWeek)
    {
      if(u.oraInizio!="00:00:00")
      {
        //inizio= u.oraInizio;
        SetoraInz(u.oraInizio);
        SetdataInz(u.oraInizio);
      } 

      if(u.oraFine!="00:00:00")
      { 
       // fine= u.oraFine;
        SetoraFin(u.oraFine);
        SetdataFin(u.oraFine)
      }
    }
  });

},[props.array,props.dayOfWeek]);

let totIn=0;  
let totFi=0;
let tot =0;
let ttt=0; 



useEffect(()=>{
  setTimeout(() => {
    const d = new Date();
    let ore = d.getHours()*60;
    let minuti = d.getMinutes();
    let tot = ore + minuti; 
    let ii =(tot/1440)*100;
    SeTtempoCon(ii);
  }, 1000);

  setTimeout(()=>{
    const d = new Date();
    if(!regex.test(oraFin))
    {
      return;
    }
    let ii= regex.exec(oraFin);
    if(ii == null)
    {
      return;
    }

    let ora = Number(ii[2]);
    let minuti = Number(ii[3]);
    let secondi = Number(ii[4]);
    d.setHours(ora, minuti, secondi);
    let hours = d.getHours()*60;
    let minutis = d.getMinutes();
    let tot = hours + minutis; 
    totFi = (tot/1440)*100;
     // oraTF(totFi); 

  },100);
  setTimeout(()=>{ 
    const d = new Date();
    if(!regex.test(oraInz))
    {
      return;
    }
    let ii= regex.exec(oraInz);
    if(ii == null)
    {
      return;
    }


      let ora = Number(ii[2]); 
      let minuti = Number(ii[3]);
      let secondi = Number(ii[4]);
      d.setHours(ora, minuti, secondi);
      let hours = d.getHours()*60;
      let minutis = d.getMinutes();
      let tot = hours + minutis; 
      totIn = (tot/1440)*100;
      SetoraTotInz(totIn);
    },100);
    setTimeout(()=>{ 
      tot = totFi - totIn;
      SetoraCors(tot); 
      ttt =100-(tot + totFi +totIn);
      SetoraTotFin(totFi + ttt);
    },100);
  },[oraFin, oraInz]);

  return( 
        <div className={"aa1"}> 
          <h2>{days[props.dayOfWeek]}</h2>
          <form className="input-group mb-3">
            <span className="input-group-text App_inzio" id="inputGroup-sizing-default">inzio</span>
            <label  className="form-label"></label>
            <DebounceInput 
              type="text" 
              minLength={8}
              debounceTimeout={300}
              className="form-control input" 
              id="input" 
              placeholder="HH:MM:SS"
              onFocus={() => setFocus(true)} 
              onBlur={() => setFocus(false)} 
              value={dataInz}
              onChange={(e)=>{dataInizio(e.target.value);
              }}/>
          </form>
          <form className="input-group mb-3" >
            <span className="input-group-text App_fine" id="inputGroup-sizing-default">fine</span>
            <label  className="form-label"></label>
            <DebounceInput 
              type="text"
              minLength={8}
              className="form-control input_1" 
              id="input_1" 
              placeholder="HH:MM:SS" 
              onFocus={() => setFocus(true)} 
              onBlur={() => setFocus(false)} 
              value={dataFin}
              onChange={(e)=>{dataFine(e.target.value);
              }}/>  
          </form> 

        
          <div className="progress barra">
            <div className="progress-bar bg-danger" role="progressbar" style={{width: oraTotInz + '%' }}  aria-valuemin={0} aria-valuemax={100}></div>
            <div className="progress-bar bg-success" role="progressbar" style={{width: oraCors + '%' }}  aria-valuemin={0} aria-valuemax={100}></div>
            <div className="progress-bar  bg-danger" role="progressbar" style={{width: oraTotFin+ '%' }} aria-valuemin={0} aria-valuemax={100}></div> 
          </div>


          <div className="progress barra1">
            <div className="progress-bar progress-bar-striped progress-bar-animated bg-info" role="progressbar" style={{width: tempoCon + '%' }}  aria-valuemin={0}aria-valuemax={100}></div>
          </div>
        </div>     
    );
  
}
//          {d.getDay()===props.dayOfWeek ? <MeteoManu/> : null}