import "./list-urls.css";

function ListUrls({urls, continuationToken, onLoadMore}) {
    return (
        <div className="url-list">
            {urls && urls.map((url) => (
                <div key={url.shortUrl} className="url-item">
                    <a href={url.shortUrl} className="short-url">
                        {url.id}
                    </a>
                    {" → "}
                    <a href={url.longUrl} className="long-url">
                        {url.longUrl}
                    </a>
                </div>
            ))}
            {continuationToken && (
                <button onClick={onLoadMore}>Load more</button>
            )}
        </div>
    );
}

export default ListUrls;