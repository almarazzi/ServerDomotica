import { useEffect, useState } from "react";


interface User{
    readonly userName: string;
    readonly role: string;
    readonly isOnline: boolean;
    readonly statoAccount: boolean;
}



export function ControlloUtenti() {
    const [users, setUsers] = useState([] as User[]);
    
    useEffect(() => {        
        let isActive = true;
        const fetchData = async () => {            
            let data = await fetch("/Login/Getlistuser" , {method: 'GET'});
            if(!isActive) return;
            var res = await data.json() as User[];
            if(!isActive) return;
            setUsers(res);
            if(isActive===true) setTimeout(()=>{fetchData();},500);
        };
        fetchData();
        return ()=>{isActive=false;}  //cleanup when component unmounts
    },[]);


    return (        
        <div className="prova">
            <table border={2} width={500}>
                <thead>
                    <tr>
                        <th className="tabella">NumeroUtente</th>
                        <th className="tabella">NomeUtente</th>
                        <th className="tabella">Ruolo</th>
                        <th className="tabella">Sospendi L'account</th>
                        <th className="tabella">Riabilita L'account</th>
                        <th className="tabella">Stato</th>
                        <th className="tabella">Online</th>
                    </tr>  
                </thead>          
                <tbody>
                    { users.map( (u, i) =>
                        <tr key={u.userName}>
                            <td className="tabella">{i}</td>
                            <td className="tabella">{u.userName}</td>
                            <td className="tabella">{u.role}</td>
                            <td className="prova2">
                                <button type="button" className={"Button btn btn-primary"} onClick={async()=>{const inv={Username: u.userName, StatoAccount: false};
                                await fetch("/Login/StatoAccount", { body: JSON.stringify(inv), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' }});
                                }}>Schiaccia qui</button>
                            </td>
                            <td className="prova2">
                                <button type="button" className="btn btn-primary Button" onClick={async()=>{const inv={Username: u.userName, StatoAccount: true};
                                await fetch("/Login/StatoAccount", { body: JSON.stringify(inv), method: "PUT", headers: { 'Content-type': 'application/json; charl set=UTF-8' }});
                                }}>Schiaccia qui</button>
                            </td>
                            <td className="tabella">{u.statoAccount ? "Abbilitato": "Sospeso"}</td>
                            <td className={"tabella"}> <div className={u.isOnline ? "isOnline": "isOffline"}></div></td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}