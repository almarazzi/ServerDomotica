import { Fragment, useState,useEffect } from "react";
import ComponenteEsp from "./componenteEsp";
import * as signalR from "@microsoft/signalr"

interface Lista {
    readonly nomeEspClient: string;
    readonly ipEsp: string;
    readonly abilitazione: boolean;
}
interface key {
    key: string;
    value: Lista;
}
export function Esp(props: { lista: key[] }) {
const [isOffline, setIsoffline] = useState<Record<string,boolean>>({});

useEffect(()=>{

    const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();
        
        con.on("StatoRelayOff",(a:String[])=>{
            setIsoffline(statoAttuale =>{
                const staotN = {...statoAttuale};
                    props.lista.forEach(u =>{
                        if(a.includes(u.key))
                        {
                            staotN[u.key] = true;
                        }else
                        {
                            delete staotN[u.key];
                        }
                    });
                    return staotN
                });
            });
        con.start().catch(err => console.error(err.toString()));

        return(()=>{
        if (con.state === signalR.HubConnectionState.Connected) {
            con.stop().catch(err => console.error("Errore SignalR stop:", err));
        }});

},[props.lista]);

    return <Fragment>
        {props.lista.map((u, i) =>
            <div className={isOffline[u.key] ? "Offline" : "Online"}>
                <ComponenteEsp key={i} Ablitazione={u.value.abilitazione} ip={u.value.ipEsp} mac={u.key} nome={u.value.nomeEspClient}  isoffline={isOffline} />
            </div>
        )}

    </Fragment>

}

export default Esp;