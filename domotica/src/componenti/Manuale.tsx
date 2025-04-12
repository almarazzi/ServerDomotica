import { Fragment, useEffect, useState,  } from "react";
import { DayManuale } from "./DayManuale";


interface Oragiorno{
    readonly oraInizio: string;
    readonly oraFine: string;
    readonly day: number;
}
interface key{
    key: string;
    value:Oragiorno[];
}
export function Manuale(props:{mac:string}){  
const [r, setr] = useState([] as Oragiorno[]);
//const [pp, setpp] = useState("");

//const a = new Date();
useEffect(() => {
    let isactive= true;
    const fetchData1 = async () => {
        let data = await fetch("/api/RelaySwitch/GetWeekProgram" ,{method:'GET',headers: { 'Content-type': 'application/json; charl set=UTF-8' }});    
        var res = await data.json() as key[];
        res.map((u,_)=>{
            if(u.key===props.mac)
            {
                setr(u.value);
                //setpp(u.value);
                r.forEach(u=>console.log(u.oraFine));
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
            <DayManuale key={i}  dayOfWeek={i}  array={r} mac={props.mac} />
            <hr/>
            </div>
            )}
    </Fragment>    
}