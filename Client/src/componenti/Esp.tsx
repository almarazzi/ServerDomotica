import { Fragment, useEffect, useState } from "react";
import { ComponenteEsp } from "./componenteEsp";

interface Lista{
    readonly nomeEspClient: string;
    readonly ipEsp : string;
    readonly abilitazione:boolean;
}
interface key{
    key: string;
    value:Lista;
}
export function Esp(){
    const [lista, Setlista] = useState([] as key[]);

    useEffect(() => {        
        let isActive = true;
        const fetchData = async () => {            
            let data = await fetch("/apiEsp/ListaEsp" , {method: 'GET'});
            if(!isActive) return;
            var res = await data.json() as key[];
            if(!isActive) return;
            Setlista(res);
            
            if(isActive===true) setTimeout(()=>{fetchData();},500);
        };
        fetchData();
        return ()=>{isActive=false;}  
    },[]);

    return<Fragment>
        {lista.map((u,i) =>
            <ComponenteEsp key={i} abilitazioe={u.value.abilitazione} ip={u.value.ipEsp} mac={u.key} nome={u.value.nomeEspClient}/>
        )}
        
    </Fragment>
    
}

