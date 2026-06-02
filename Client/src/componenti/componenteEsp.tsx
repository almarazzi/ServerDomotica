import { useCallback, useEffect, useState } from "react";
import { DebounceInput } from "react-debounce-input";
import { Link } from "react-router-dom";
import * as signalR from "@microsoft/signalr"


interface Tutto {
    state: boolean,
    macricever: string
}
export function ComponenteEsp(props: { mac: string, ip: string, Ablitazione: boolean, nome: string, isoffline : (Record<string, boolean>) }) {
    const [nomeEsp, SetnomeEsp] = useState("");
    const [mac, Setmac] = useState("");
    const [focus, setFocus] = useState(false);
    const [Ablitazione, setAbilitazione] = useState(false);
    //const [Isoffline, setIsoffline] = useState<Record<string,boolean>>({});
    const [M, setM] = useState(false);
    const [A, setA] = useState(false);

    useEffect(() => {
        const api = async () => {
            await fetch("/apiEsp/NomeEsp", { body: JSON.stringify({ mac: mac, nomeEsp: nomeEsp }), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
        }
        if (focus && mac !== "") {
            api();
        }
    }, [nomeEsp, mac, focus]);

    useEffect(() => {
        const api = async () => {
            await fetch("/apiEsp/abilitazione", { body: JSON.stringify({ abilitazione: Ablitazione, mac: mac }), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
        }
        if (Ablitazione !== null && mac !== "") {
            api();
        }
    }, [Ablitazione, mac]);

    const y = useCallback(async () => {

        const inv = { stateProgrammManu: !M, macricever: props.mac };
        await fetch("/api/RelaySwitch/stateProgrammManu", { method: "PUT", body: JSON.stringify(inv), headers: { 'Content-type': 'application/json; charl set=UTF-8' } });
    }, [M]);
    useEffect(() => {
         const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();
                
                con.on("GetProgmmaManu",(a:Tutto[])=>{
                     a.map((u, _) => {
                        if (u.macricever === props.mac) {
                            setM(u.state)
                        }
                    })
                    });
                con.start().catch(err => console.error(err.toString()));
        
            return(()=>{
                if (con.state === signalR.HubConnectionState.Connected) {
                    con.stop().catch(err => console.error("Errore SignalR stop:", err));
                }
            });
    }, []);

    const p1 = useCallback(async () => {
        const inv = { stateProgrammAuto: !A, macricever: props.mac };
        await fetch("/api/RelaySwitch/stateProgrammAuto", { method: "PUT", body: JSON.stringify(inv), headers: { 'Content-type': 'application/json; charl set=UTF-8' } })
    }, [A]);

    useEffect(() => {
        const con = new signalR.HubConnectionBuilder().withUrl("/relaySwitchHub").build();
                
                con.on("GetProgmmaAuto",(a:Tutto[])=>{
                     a.map((u, _) => {
                        if (u.macricever === props.mac) {
                            setA(u.state)
                        }
                    })
                    });
                con.start().catch(err => console.error(err.toString()));
        
            return(()=>{
                if (con.state === signalR.HubConnectionState.Connected) {
                    con.stop().catch(err => console.error("Errore SignalR stop:", err));
                }
            });
    }, []);

    return (
        <div className="ccccc" >
            <DebounceInput
                type="text"
                minLength={4}
                debounceTimeout={500}
                className="form-control input_ESP"
                id="input"
                placeholder="Nome Esp"
                onFocus={() => setFocus(true)}
                onBlur={() => setFocus(false)}
                value={(focus === false ? props.nome : nomeEsp)}//(focus === false ? u.value.nomeEspClient : nomeEsp)
                onChange={(e) => {
                    SetnomeEsp(e.target.value);
                    Setmac(props.mac);
                }} />
            <div className="form-check input_check">
                <input className="form-check-input" type="checkbox" checked={props.Ablitazione} onChange={(e) => { setAbilitazione(e.target.checked); Setmac(props.mac); }} id="abiliatazione" required />
                <label form="abiliatazione"> Abilitazione {props.nome}</label>
            </div>

            <div className="ip">IP:{props.ip}</div>
            <div className="mac">MAC:{props.mac}</div>
            <div className="componenteAutoManu">
                <input className="form-check-input casellaAuto" type="checkbox" checked={A} onChange={p1} id="invalidCheck1" required />


                <Link to={ (!props.isoffline[props.mac] ? "/Automatico/" + props.mac : "")} className="bottoneAuto dropdown-item">
                    <button  type="button" className="btn btn-outline-primary ">Automatico</button>
                </Link>

                <Link to={(!props.isoffline[props.mac] ? "/Manuale/" + props.mac: "")} className="bottoneManu dropdown-item">
                    <button  type="button" className="btn btn-outline-primary ">Manuale</button>
                </Link>

                <input className="form-check-input casellaManu" type="checkbox" checked={M} onChange={y} id="invalidCheck" required />
            </div>
        </div>

    );
}
export default ComponenteEsp;
