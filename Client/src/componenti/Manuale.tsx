import { useEffect, useState, Fragment } from "react";
import * as signalR from "@microsoft/signalr"
interface Tutto {
  state: boolean,
  macricever: string
}
export function Manuale(props: { mac: string }) {
  const [state, stateOn] = useState(false);



  useEffect(()=>{


    const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();

    con.on("CambioStatoRealy",(a:Tutto[])=>{
      a.forEach(element => {
          if(element.macricever === props.mac)
            stateOn(element.state)
      });
    });
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

  },[props.mac,stateOn])
  

  return (
    <Fragment>
      <div className="Manuale1">
        <button type="button" className={"Buttone1  btn btn-" + (state === true ? "primary" : "secondary")} onClick={async () => {
          await fetch("/api/RelaySwitch/SetState", { method: "PUT", body: JSON.stringify({ state: true, macricever: props.mac }), headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
          stateOn(true);
        }}> ON</button>

        <button type="button" className={"Buttone2 btn btn-" + (state === false ? "primary" : "secondary")} onClick={async () => {
          await fetch("/api/RelaySwitch/SetState", { method: "PUT", body: JSON.stringify({ state: false, macricever: props.mac }), headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
          stateOn(false);
        }}> OFF</button>
      </div>
    </Fragment>
  );
}
export default Manuale;