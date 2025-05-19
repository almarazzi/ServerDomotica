import { Fragment, useEffect, useState,  } from "react";
import  DayAutomatico from "./DayAutomatico";


interface Oragiorno{
    readonly oraInizio: string;
    readonly oraFine: string;
    readonly day: number;
}
interface key{
    key: string;
    value:Oragiorno[];
}
export function Automatico(props:{mac:string}){  
const [r, setr] = useState([] as Oragiorno[]);

useEffect(() => {
    let isactive= true;
    const fetchData1 = async () => {
        let data = await fetch("/api/RelaySwitch/GetWeekProgram" ,{method:'GET'});    
        var res = await data.json() as key[];
        res.map((u,_)=>{
            if(u.key===props.mac)
            {
                setr(u.value);
            }
        })
        if(isactive)
        {
            setTimeout(() =>{
                fetchData1();
            },500);
        }
    };
    fetchData1();
    return() => {
        isactive=false;
    };
},[]);
    return <Fragment>
        {
        [...Array(7)].map((_,i) => 
            <div>
            <DayAutomatico key={i}  dayOfWeek={i}  array={r} mac={props.mac} />
            <hr/>
            </div>
            )}
    </Fragment>    
}
export default Automatico;