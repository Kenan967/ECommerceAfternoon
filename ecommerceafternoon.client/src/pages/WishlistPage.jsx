import { useEffect, useState } from "react";
import {
    Box,
    Button,
    Card,
    CardContent,
    Container,
    Grid,
    Typography
} from "@mui/material";
import { Link } from "react-router-dom";
import {
    getWishlist,
    removeFromWishlist
} from "../services/wishlistService";

function WishlistPage() {
    const [items, setItems] = useState([]);

    const loadWishlist = async () => {
        try {
            const data = await getWishlist();
            setItems(data);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        loadWishlist();
    }, []);

    const handleRemove = async (productId) => {
        try {
            await removeFromWishlist(productId);
            setItems((current) =>
                current.filter((item) => item.productId !== productId)
            );
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Typography variant="h3" fontWeight="bold" sx={{ mb: 4 }}>
                My Wishlist
            </Typography>

            {items.length === 0 ? (
                <Typography color="text.secondary">
                    Your wishlist is empty.
                </Typography>
            ) : (
                <Grid container spacing={3}>
                    {items.map((item) => (
                        <Grid key={item.id} size={{ xs: 12, sm: 6, md: 4 }}>
                            <Card>
                                <Box
                                    component="img"
                                    src={item.imageUrl}
                                    alt={item.name}
                                    sx={{
                                        width: "100%",
                                        height: 240,
                                        objectFit: "cover"
                                    }}
                                />

                                <CardContent>
                                    <Typography variant="h6" fontWeight="bold">
                                        {item.name}
                                    </Typography>

                                    <Typography sx={{ mt: 1 }}>
                                        ${item.price}
                                    </Typography>

                                    <Box sx={{ display: "flex", gap: 1, mt: 2 }}>
                                        <Button
                                            component={Link}
                                            to={`/products/${item.productId}`}
                                            variant="outlined"
                                        >
                                            View
                                        </Button>

                                        <Button
                                            color="error"
                                            variant="outlined"
                                            onClick={() => handleRemove(item.productId)}
                                        >
                                            Remove
                                        </Button>
                                    </Box>
                                </CardContent>
                            </Card>
                        </Grid>
                    ))}
                </Grid>
            )}
        </Container>
    );
}

export default WishlistPage;
