import { Fragment, useEffect, useState, } from "react";
import DayAutomatico from "./DayAutomatico";
import * as signalR from "@microsoft/signalr"


interface Oragiorno {
    readonly oraInizio: string;
    readonly oraFine: string;
    readonly day: number;
}
interface key {
    key: string;
    value: Oragiorno[];
}
export function Automatico(props: { mac: string }) {
    const [r, setr] = useState([] as Oragiorno[]);
    
    useEffect(() => {

        const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();
    
          con.on("ProgrammaSettimanale",(a:key[])=>{
            a.map((u, _) => {
                if (u.key === props.mac) {
                    setr(u.value);
                }
            })
        });
        con.start().catch(err => console.error(err.toString()));
    
        return(()=>{ 
          if (con.state === signalR.HubConnectionState.Connected) {
            con.stop().catch(err => console.error("Errore SignalR stop:", err));
        }});
    
    }, []);

useEffect(() => {
   
    const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();
    
          con.on("StatoRelayOff",(a:String[])=>{
           if(a.includes(props.mac)) {
              window.location.href = "/";
            }
        });
        con.start().catch(err => console.error(err.toString()));
    
        return(()=>{ 
          if (con.state === signalR.HubConnectionState.Connected) {
            con.stop().catch(err => console.error("Errore SignalR stop:", err));
          }});
    
  },[props.mac]);
  
    return <Fragment>
        {
            [...Array(7)].map((_, i) =>
                <div>
                    <DayAutomatico key={i} dayOfWeek={i} array={r} mac={props.mac} />
                    <hr />
                </div>
            )}
    </Fragment>
}
export default Automatico;