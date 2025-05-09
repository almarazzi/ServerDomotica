import { useCallback, useEffect, useState } from "react";
import OfflineNonAbilitato from "./OfflineNonAbilitato.png";
import OfflineAblitato from './OfflineAbilitato.png';
import OnlineAblitato from "./OnlineAbilitato.png";


interface User{
    readonly userName: string;
    readonly role: string;
    readonly isOnline: boolean;
    readonly statoAccount: boolean;
}



export function ControlloUtenti(props:{setSizeG: (t:boolean)=>void}) {
    const [users, setUsers] = useState([] as User[]);
    const [Abilitazione, setAbilitazione] = useState<{[Nomeu:string]:boolean}>({});
    const [Immagine, setImmagine] = useState<{[Nomeu:string]:string}>({});
    useEffect(()=>{
        const larghezza = ()=>{
        if(window.innerWidth<=1000)
        {
            props.setSizeG(true);
        }else
        {
            props.setSizeG(false);
        }
        setTimeout(()=>{
            larghezza();
        },1000);
    }
    larghezza();
    },[])
    useEffect(() => {        
        let isActive = true;
        const fetchData = async () => {            
            let data = await fetch("/Login/Getlistuser" , {method: 'GET'});
            if(!isActive) return;
            var res = await data.json() as User[];
            if(!isActive) return;
            setUsers(res);
            if(isActive===true) 
            {
                setTimeout(()=>{
                    fetchData();
                },500);
            }
        };
        fetchData();
        return ()=>{isActive=false;}  //cleanup when component unmounts
    },[]);


    const p1 = useCallback(async (nomeU: string) => {
        const inv = { Username: nomeU, StatoAccount: !Abilitazione[nomeU] };
        await fetch("/Login/StatoAccount", { body: JSON.stringify(inv), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' } });

    }, [Abilitazione]);

    useEffect(() => {
        users.map((u, _) => {
            setAbilitazione(StatoAttuale => ({
                ...StatoAttuale,
                [u.userName]: u.statoAccount
            }));
        });
    },[users]);

    useEffect(() => {
        users.map((u, _) => {
            if (u.isOnline === true && u.statoAccount === true) {
                setImmagine(StatoAttuale => ({
                    ...StatoAttuale,
                    [u.userName]: OnlineAblitato
                }));
            }
            if (u.isOnline === false && u.statoAccount === true) {
                setImmagine(StatoAttuale => ({
                    ...StatoAttuale,
                    [u.userName]: OfflineAblitato
                }));
            }
            if (u.isOnline === false && u.statoAccount === false) {
                setImmagine(StatoAttuale => ({
                    ...StatoAttuale,
                    [u.userName]: OfflineNonAbilitato
                }));
            }
        });
    }, [users]);

    return (        
        <div className="prova">
            <table border={2} width={500}>
                <thead>
                    <tr>
                        <th className="tabella">NumeroUtente</th>
                        <th className="tabella">NomeUtente</th>
                        <th className="tabella">Ruolo</th>
                        <th className="tabella">Sospendi L'account</th>
                        <th className="tabella">Online/Stato</th>
                    </tr>  
                </thead>          
                <tbody>
                    { users.map( (u, i) =>
                        <tr key={u.userName}>
                            <td className="tabella">{i}</td>
                            <td className="tabella">{u.userName}</td>
                            <td className="tabella">{u.role}</td>
                            <td className="tabella">
                                <input className="form-check-input" type="checkbox" checked={!Abilitazione[u.userName]} onChange={()=>p1(u.userName)} id="invalidCheck1" required /> 
                            </td>
                            <td className={"tabella"}> <img src={Immagine[u.userName]} className="ridotto"></img></td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}