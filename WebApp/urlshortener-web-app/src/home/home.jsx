import "./home.css";
import { useState, useEffect } from "react";
import { useMsal } from "@azure/msal-react";
import LogoutButton from "../logout-button/logout-button";
import ListUrls from "../list-urls/list-urls";
import UrlForm from "../url-form/url-form";
import axios from "axios";

function Home() {
    const scope = `api://${import.meta.env.VITE_APP_CLIENT_ID}/Urls.Read`;
    const apiEndpoint = import.meta.env.VITE_APP_API_ENDPOINT;

    const { instance, accounts } = useMsal();

    const [data, setData] = useState({
        initialized: false,
        urls: [],
        continuationToken: null
    });

    const handleLogout = () => {
        instance.logoutRedirect();
    };

    const getToken = async () => {
        const request = {
            scopes: [`openid profile ${scope}`],
            account: accounts[0]
        };

        const response = await instance.acquireTokenSilent(request);

        return response.accessToken;
    };

    const fetchUrls = async (loadMore = false) => {
        const token = await getToken();

        const response = await axios.get(`${apiEndpoint}/api/urls`, {
            headers: {
                Authorization: `Bearer ${token}`
            },
            params: {
                continuation: loadMore ? data.continuationToken : null,
                pageSize: 5
            }
        });

        setData(prev => ({
            initialized: true,
            urls: loadMore
                ? [...prev.urls, ...response.data.urls]
                : response.data.urls,
            continuationToken: response.data.continuationToken
        }));
    };

    const handleLoadMore = () => {
        fetchUrls(true);
    };

    const handleSubmit = async (longUrl) => {
        const token = await getToken();

        await axios.post(
            `${apiEndpoint}/api/urls`,
            { LongUrl: longUrl },
            {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            }
        );

        // Refresh the list after creating the URL.
        await fetchUrls(false);
    };

    useEffect(() => {
        if (!data.initialized) {
            fetchUrls();
        }
    }, []);

    return (
        <div className="container">
            <h1>Dometrain URL Shortener</h1>

            <div className="header">
                <LogoutButton onLogout={handleLogout} />
            </div>

            <UrlForm onSubmit={handleSubmit} />

            <ListUrls
                urls={data.urls}
                continuationToken={data.continuationToken}
                onLoadMore={handleLoadMore}
            />
        </div>
    );
}

export default Home;
