import { Fragment, useState,useEffect } from "react";
import ComponenteEsp from "./componenteEsp";

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


    useEffect(() => {
        let isactive = true;
        const fetchData = async () => {
            let data = await fetch("/apiEsp/StatoRelay", { method: 'GET' });
            var res = await data.json() as string[];
            if (isactive) {
                {
                    if (res.includes(props.lista.map(u=>u.key).toString()))
                    {
                       setIsoffline(statoAttuale =>({
                        ...statoAttuale,
                        [props.lista.map(u=>u.key).toString()]: true

                       }));
                    }else {
                      delete isOffline[props.lista.map(u=>u.key).toString()];
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
    }, [props.lista]);
    return <Fragment>
        {props.lista.map((u, i) =>
            <div className={isOffline[u.key] ? "Offline" : "Online"}>
                <ComponenteEsp key={i} Ablitazione={u.value.abilitazione} ip={u.value.ipEsp} mac={u.key} nome={u.value.nomeEspClient}  isoffline={isOffline} />
            </div>
        )}

    </Fragment>

}

export default Esp;