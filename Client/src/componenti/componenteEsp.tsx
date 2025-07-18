import { useCallback, useEffect, useState } from "react";
import { DebounceInput } from "react-debounce-input";
import { Link } from "react-router-dom";


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
        let isactive = true;
        const fetchData = async () => {
            let data = await fetch("/api/RelaySwitch/GetProgrammManu", { method: 'GET' });
            var res = await data.json() as Tutto[];
            if (isactive) {
                {
                    res.map((u, _) => {
                        if (u.macricever === props.mac) {
                            setM(u.state)
                        }
                    })
                }
                setTimeout(() => {
                    fetchData();

                }, 500);
            }
        };
        fetchData();
        return () => {
            isactive = false;
        };
    }, []);

    const p1 = useCallback(async () => {
        const inv = { stateProgrammAuto: !A, macricever: props.mac };
        await fetch("/api/RelaySwitch/stateProgrammAuto", { method: "PUT", body: JSON.stringify(inv), headers: { 'Content-type': 'application/json; charl set=UTF-8' } })
    }, [A]);

    useEffect(() => {
        let isactive = true;
        const fetchData1 = async () => {
            let data = await fetch("/api/RelaySwitch/GetProgrammAuto", { method: 'GET' });
            let res = await data.json() as Tutto[];
            if (isactive) {
                {
                    res.map((u, _) => {
                        if (u.macricever === props.mac) {
                            setA(u.state)
                        }
                    })
                }
                setTimeout(() => {
                    fetchData1();
                }, 500);
            }
        };
        fetchData1();
        return () => {
            isactive = false;
        };
    }, []);

    useEffect(() => {
        let isactive = true;
        const fetchData = async () => {
            let data = await fetch("/apiEsp/StatoRelay", { method: 'GET' });
            var res = await data.json() as string[];
            if (isactive) {
                {
                    if (res.includes(props.mac)) 
                    {
                        props.isoffline[props.mac] =true
                    }else {
                       delete props.isoffline[props.mac];
                    }

                }
                setTimeout(() => {
                    fetchData();
                }, 500);
            }
        };
        fetchData();
        return () => {
            isactive = false;
        };
    }, [props.mac]);
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
            <div className="ip">Stato Relay:{(props.isoffline[props.mac] ? "Offline" : "Online")}</div>
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