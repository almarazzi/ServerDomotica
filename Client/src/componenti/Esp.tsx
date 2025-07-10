import { Fragment, useState } from "react";
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
const [isOffline] = useState<Record<string,boolean>>({});
    return <Fragment>
        {props.lista.map((u, i) =>
            <div className={isOffline[u.key] ? "Offline" : "Online"}>
                <ComponenteEsp key={i} Ablitazione={u.value.abilitazione} ip={u.value.ipEsp} mac={u.key} nome={u.value.nomeEspClient}  isoffline={isOffline} />
            </div>
        )}

    </Fragment>

}

export default Esp;