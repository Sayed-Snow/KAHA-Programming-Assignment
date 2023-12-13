import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";

const TableComponent = () => {
    const [sortField, setSortField] = useState("");
    const [sortDirection, setSortDirection] = useState("asc");
    const [CountryData,setCountryData] = useState([]);
    const [loading,setLoading] = useState(true)

    useEffect(() => {
        async function fetchData() {
          const response = await fetch("api/countries/top5");
          const data = await response.json();
          setCountryData(data);
          setLoading(false);
        }
        fetchData();
      }, []);
  
    const handleSort = (field) => {
      if (field === sortField) {
        setSortDirection(sortDirection === "asc" ? "desc" : "asc");
      } else {
        setSortField(field);
        setSortDirection("asc");
      }
    };


    const sortedData = CountryData.slice().sort((a, b) => {
      if (sortField === "name") {
        return a.name.localeCompare(b.name) * (sortDirection === "asc" ? 1 : -1);
      } else if (sortField === "capital") {
        return a.capital.localeCompare(b.capital) * (sortDirection === "asc" ? 1 : -1);
      } else if (sortField === "population") {
        return (a.population - b.population) * (sortDirection === "asc" ? 1 : -1);
      } else {
        return 0;
      }
    });

    if (loading) {
        return <div>Loading...</div>;
      }
  
    return (
      <table className="table table-striped">
        <thead>
          <tr>
            <th>Name</th>
            <th>Capital</th>
            <th onClick={() => handleSort("population")}>Population</th>
            <th>Latitude</th>
            <th>Longitude</th>
            <th>Info</th>
          </tr>
        </thead>
        <tbody>
          {sortedData.map((country, index) => (
            <tr key={index} >
              <td>{country.name}</td>
              <td>{country.capital}</td>
              <td>{country.population.toLocaleString()}</td>
              <td>{country.latitude}</td>
              <td>{country.longitude}</td>
              <td><Link to={`/country/${country.name}`}>More Info</Link>  </td>
            </tr>
          ))}
        </tbody>
       
      </table>

    );
  };

  
  export default TableComponent;
  