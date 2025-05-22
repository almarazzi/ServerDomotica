import { Fragment } from "react";
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


    return <Fragment>
        {props.lista.map((u, i) =>
            <ComponenteEsp key={i} abilitazioe={u.value.abilitazione} ip={u.value.ipEsp} mac={u.key} nome={u.value.nomeEspClient} />
        )}

    </Fragment>

}

export default Esp;