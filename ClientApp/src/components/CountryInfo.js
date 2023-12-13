import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

const CountryInfo = () => {
    const { country } = useParams();
    const [countryData,setCountryData] = useState([]);
    const [loading,setLoading] = useState(true)

    useEffect(() => {
        async function fetchData() {
          const response = await fetch(`api/countries/summary/${country}`);
          const data = await response.json();
          setCountryData(data);
          setLoading(false);
        }
        fetchData();
      }, []);

  if(loading){
    return(
        <div>Loading</div>
    )
  }

  return (
    <div className="country-info">
      <div className="main-card">
        <img src={countryData.flag} alt={countryData.name} />
        <h3>{countryData.name}</h3>
        <p>Capital: {countryData.capital}</p>
        <p>Population: {countryData.population}</p>
        <p>Time Zone: {countryData.timezone}</p>
          <ul className="details">
            <li>Starts on: {countryData.startOfWeek}</li>
            <li>Currency: {countryData.currency}</li>
            <li>Languages: {countryData.languages}</li>
            <li>Car side: {countryData.carSide}</li>
            <li>Location: {`Latitude: ${countryData.latitude}, Longitude: ${countryData.longitude}`}</li>
            <li>Sunrise: {countryData.sunrise}</li>
            <li>Sunset: {countryData.sunset}</li>
          </ul>
      </div>
    </div>
  );
};

export default CountryInfo;
