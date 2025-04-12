import { useEffect, useState, Fragment } from "react";

interface Tutto {
  state : boolean,
  macricever: string
}
export function Automatico (props:{mac:string}){
    const [state, stateOn ] = useState(false);

    useEffect(() => {
      let isactive= true;

        const fetchData = async () => {
          
          let data = await fetch("/api/RelaySwitch/GetState" , {method: 'GET',headers: { 'Content-type': 'application/json; charl set=UTF-8' }});    
          var res = await data.json() as Tutto[];
        
          if(isactive)
          {
            res.map((u,_)=>{
            if(u.macricever===props.mac)
            stateOn(u.state);
            })
            setTimeout(() =>{
              fetchData();
            },500);
          }
        };
        
        fetchData(); 
        return() => {
          isactive=false;
        };
    },[]);

    return(
      <Fragment>
        <div className="Manuale1">
            <button type="button" className={ "Buttone1  btn btn-" + (state === true ?  "primary" : "secondary") }  onClick={async ()=>{ 
              await fetch("/api/RelaySwitch/SetState", {  method: "PUT", body: JSON.stringify({state:true, macricever:props.mac }) ,headers: { 'Content-type': 'application/json; charl set=UTF-8' }});
            }}> ON</button>

            <button type="button" className={ "Buttone2 btn btn-" + (state === false ?  "primary" : "secondary") }  onClick={async ()=>{ 
              await fetch("/api/RelaySwitch/SetState" , { method: "PUT",body: JSON.stringify({state:false, macricever:props.mac }), headers: { 'Content-type': 'application/json; charl set=UTF-8' }});
            }}> OFF</button>
        </div>
        </Fragment>
    );
}   

export default Automatico;