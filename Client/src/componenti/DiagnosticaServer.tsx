import { useEffect, useState } from "react";

interface Spazio{
    nome:""
    spazioLibero:0
    spazioTotale:0
}



export function DiagnosticaServer(){
const[Nome,Setnome] = useState("")
const[SpazioLibero,SetSpazioLibero] = useState(0)
const[SpazioTotale,SetSpazioTotale] = useState(0)

useEffect(()=>{
let isActive = true

const api = async ()=>{
    let apid = await fetch("/apiPC/SpazioDig",{method : "GET"});
    let res = await apid.json() as Spazio;
    if(isActive )
    {
        Setnome(res.nome);
        SetSpazioLibero(res.spazioLibero);
        SetSpazioTotale(res.spazioTotale);
        
    }
};

api();

return ()=>{isActive = false}
},[SpazioLibero, SpazioTotale]);
    let SpazioOccupato = SpazioTotale -SpazioLibero;
    let tot = (SpazioOccupato/SpazioTotale) *100;

    return(
        <div className="divDiagServerSpazio">
            <div className="Diag fw-bolder" >Spazio sul disco PC server: Dsco Locale({Nome})</div>
            <div className="Diag fw-bolder" >Spazio Tot sul disco PC server: {SpazioTotale} gb</div>
            <div className="Diag fw-bolder" >Spazio Libero sul disco PC server: {SpazioLibero} gb</div>
            <div className="Diag fw-bolder" >Spazio Occupato sul disco PC server: {SpazioOccupato} gb</div>
            <div className="progress">
            <div className=" progress-bar bg-success" role="progressbar" style={{ width: tot.toFixed(2) + '%' }} aria-valuemin={0} aria-valuemax={100}></div>
            </div>
        </div>
    );
};

export default DiagnosticaServer;